using Steamworks;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

// Explicit publication only. Uses the signed-in Steam client, never starts the game.
const uint App = 4348910;
const ulong ForbiddenItem = 3836818025;
if (args.Length != 2 || args[1] is not ("--check" or "--publish" or "--schinese-only"))
{
    Console.Error.WriteLine("WorkshopPublisher <independent workspace> --check|--publish|--schinese-only");
    return 2;
}
var workspace = Path.GetFullPath(args[0]);
var configPath = Path.Combine(workspace, "workshop.json");
var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));
if (config.AppId != App || config.ModId != "bottle_and_machine_icon" || config.ItemId == ForbiddenItem)
    throw new Exception("Independent workshop identity mismatch.");
var content = Path.Combine(workspace, "content");
var expected = new[] { "BottleAndMachineIcon.dll", "manifest.xml", "使用说明.txt" };
if (Directory.GetDirectories(content).Length != 0 || !Directory.GetFiles(content).Select(Path.GetFileName).Order().SequenceEqual(expected.Order()))
    throw new Exception("Content must contain exactly the three runtime files.");
var hashes = expected.ToDictionary(f => f, f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(content, f)))));
if (hashes.Any(p => !config.Files.TryGetValue(p.Key, out var hash) || hash != p.Value) || config.Files.Count != 3)
    throw new Exception("Prepared content hashes differ.");
var manifest = XDocument.Load(Path.Combine(content, "manifest.xml"));
if (manifest.Root.Element("ID").Value != config.ModId || manifest.Root.Element("ModVersion").Value != config.Version)
    throw new Exception("Manifest identity/version mismatch.");
var description = File.ReadAllText(Path.Combine(workspace, "description.txt")).TrimEnd('\r', '\n');
var note = File.ReadAllText(Path.Combine(workspace, "changenote.txt")).TrimEnd('\r', '\n');
var preview = Path.Combine(workspace, "preview.png");
if (string.IsNullOrWhiteSpace(config.Title) || string.IsNullOrWhiteSpace(description) || Encoding.UTF8.GetByteCount(description) >= 8000
    || !File.Exists(preview) || new FileInfo(preview).Length >= 1_000_000)
    throw new Exception("Invalid title, description or cover.");
if (args[1] == "--check") { Console.WriteLine("Independent local preflight PASS; no Steam connection or upload."); return 0; }
var receiptPath = Path.Combine(workspace, $"upload-result-{config.Version}.json");
var previewHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(preview)));
if (args[1] == "--schinese-only")
{
    using var prior = JsonDocument.Parse(File.ReadAllText(receiptPath));
    var root = prior.RootElement;
    if (root.GetProperty("publishedfileid").GetUInt64() != config.ItemId
        || !root.GetProperty("stages").EnumerateArray().Any(s => s.TryGetProperty("language", out var l)
            && l.GetString() == "english" && s.GetProperty("result").GetString() == "k_EResultOK")
        || hashes.Any(p => root.GetProperty("files").GetProperty(p.Key).GetString() != p.Value))
        throw new Exception("Chinese-only retry requires a successful English upload of these files.");
}
var stages = new List<object>();
void Save(string status) => File.WriteAllText(receiptPath, JsonSerializer.Serialize(new {
    version = config.Version, appid = App, publishedfileid = config.ItemId, status,
    updatedAt = DateTimeOffset.Now, files = hashes, previewSHA256 = previewHash, stages
}, new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(true));
Environment.SetEnvironmentVariable("SteamAppId", App.ToString());
Environment.SetEnvironmentVariable("SteamGameId", App.ToString());
bool initialized = false;
try
{
    if (!(initialized = SteamAPI.Init()) || !SteamUser.BLoggedOn()) throw new Exception("Steam must be running and signed in.");
    if (SteamUtils.GetAppID().m_AppId != App) throw new Exception("Steam AppID mismatch.");
    Console.WriteLine($"Steam initialized: AppID={App}; owner={SteamUser.GetSteamID()}");
    T Wait<T>(SteamAPICall_t call) where T : struct
    {
        bool done = false, failed = false;
        T result = default;
        using var callback = CallResult<T>.Create((r, io) => { result = r; failed = io; done = true; });
        callback.Set(call);
        var timer = Stopwatch.StartNew();
        while (!done && timer.Elapsed < TimeSpan.FromMinutes(4)) { SteamAPI.RunCallbacks(); Thread.Sleep(100); }
        if (!done || failed) throw new Exception($"{typeof(T).Name}: timeout/IO failure; inspect state before retrying.");
        return result;
    }
    if (config.ItemId == 0)
    {
        if (args[1] == "--schinese-only") throw new Exception("Cannot localize an item that has not been created.");
        // Record the ID immediately so a retry can never create a duplicate after successful creation.
        Save("creating");
        var created = Wait<CreateItemResult_t>(SteamUGC.CreateItem(new AppId_t(App), EWorkshopFileType.k_EWorkshopFileTypeCommunity));
        stages.Add(new { phase = "create", result = created.m_eResult.ToString(), legalAgreementRequired = created.m_bUserNeedsToAcceptWorkshopLegalAgreement });
        if (created.m_eResult != EResult.k_EResultOK) throw new Exception("CreateItem failed: " + created.m_eResult);
        config.ItemId = created.m_nPublishedFileId.m_PublishedFileId;
        File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }), new UTF8Encoding(true));
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(workspace), "docs", "WorkshopItemId.txt"), config.ItemId + Environment.NewLine, new UTF8Encoding(true));
        Save("created");
        Console.WriteLine("Created independent workshop item: " + config.ItemId);
        if (created.m_bUserNeedsToAcceptWorkshopLegalAgreement) throw new Exception("Steam Workshop legal agreement requires the owner's acceptance.");
    }
    else
    {
        var query = SteamUGC.CreateQueryUGCDetailsRequest(new[] { new PublishedFileId_t(config.ItemId) }, 1);
        try
        {
            var response = Wait<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(query));
            if (response.m_eResult != EResult.k_EResultOK || response.m_unNumResultsReturned != 1 || !SteamUGC.GetQueryUGCResult(query, 0, out var details)
                || details.m_eResult != EResult.k_EResultOK || details.m_nConsumerAppID.m_AppId != App
                || details.m_ulSteamIDOwner != SteamUser.GetSteamID().m_SteamID)
                throw new Exception("Existing item identity/ownership check failed.");
        }
        finally { SteamUGC.ReleaseQueryUGCRequest(query); }
    }
    if (config.ItemId == 0 || config.ItemId == ForbiddenItem) throw new Exception("Unsafe workshop item ID.");
    foreach (var language in args[1] == "--schinese-only" ? new[] { "schinese" } : new[] { "english", "schinese" })
    {
        var handle = SteamUGC.StartItemUpdate(new AppId_t(App), new PublishedFileId_t(config.ItemId));
        if (!SteamUGC.SetItemUpdateLanguage(handle, language) || !SteamUGC.SetItemTitle(handle, config.Title) || !SteamUGC.SetItemDescription(handle, description))
            throw new Exception("Set language/title/description failed: " + language);
        if (language == "english")
        {
            if (hashes.Any(p => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(content, p.Key)))) != p.Value))
                throw new Exception("Content changed before upload.");
            if (!SteamUGC.SetItemContent(handle, content) || !SteamUGC.SetItemPreview(handle, preview)
                || !SteamUGC.SetItemVisibility(handle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic))
                throw new Exception("Set content/preview/public visibility failed.");
        }
        Console.WriteLine($"Submitting {language}; content={language == "english"} ...");
        Save("in-progress");
        var submitted = Wait<SubmitItemUpdateResult_t>(SteamUGC.SubmitItemUpdate(handle, language == "english" ? note : null));
        stages.Add(new { phase = "submit", language, result = submitted.m_eResult.ToString(), legalAgreementRequired = submitted.m_bUserNeedsToAcceptWorkshopLegalAgreement });
        Save("in-progress");
        if (submitted.m_eResult != EResult.k_EResultOK || submitted.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            throw new Exception("Submission failed or legal agreement required: " + submitted.m_eResult);
        Console.WriteLine(language + ": Steam submission succeeded.");
    }
    Save("submitted");
    Console.WriteLine("https://steamcommunity.com/sharedfiles/filedetails/?id=" + config.ItemId);
    return 0;
}
catch (Exception e) { Save("incomplete"); Console.Error.WriteLine(e.Message); return 1; }
finally { if (initialized) SteamAPI.Shutdown(); }

sealed class Config
{
    public uint AppId { get; set; }
    public string ModId { get; set; }
    public ulong ItemId { get; set; }
    public string Version { get; set; }
    public string Title { get; set; }
    public Dictionary<string, string> Files { get; set; }
}
