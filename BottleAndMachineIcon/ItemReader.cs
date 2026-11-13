using System.Collections.Generic;
using System.Linq;

namespace BottleAndMachineIcon
{
    internal static class ItemReader
    {
        internal static List<GameItem> Scan()
        {
            var found = new List<GameItem>();
            if (!PlayerStore.IsInstanceExist()) return found;
            var seen = new HashSet<GraphNodeStorage>();
            foreach (var item in PlayerStore.Instance.FindAllItem(true)) Visit(item, found, seen, 0);
            return found;
        }

        static void Visit(GraphNodeStorage node, List<GameItem> found, HashSet<GraphNodeStorage> seen, int depth)
        {
            if (node == null || depth > 24 || !seen.Add(node)) return;
            if (node is GameItem item && GeneralHelper.IsItemOwned(item)) found.Add(item);
            foreach (var child in node.children.ToArray()) Visit(child, found, seen, depth + 1);
        }

        internal static GameItem WorkPot(GameItem machine)
        {
            if (machine == null || (machine is GameItemElement rendered && rendered.IsDestroyed())
                || !(machine.contentWindow?.childElement is MachineryPanelRow row)) return null;
            return FindSlot(row.GetPanel("container")?.content, new HashSet<object>(), 0)?.childItem;
        }

        // Only traverse the machine's own UI tree; never descend into a contained item's slots.
        static GameSlotInventory FindSlot(object element, HashSet<object> seen, int depth)
        {
            if (element == null || depth > 24 || !seen.Add(element)) return null;
            if (element is GameSlotInventory slot) return slot;
            if (element is GameItem) return null;
            if (element is MachineryIntake intake) return FindSlot(intake.content, seen, depth + 1);
            if (element is MachineryPanel panel) return FindSlot(panel.content, seen, depth + 1);
            if (element is MachineryStack stack)
                foreach (var child in stack.elements)
                {
                    var match = FindSlot(child, seen, depth + 1);
                    if (match != null) return match;
                }
            if (element is GraphNodeStorage node)
                foreach (var child in node.children.ToArray())
                {
                    var match = FindSlot(child, seen, depth + 1);
                    if (match != null) return match;
                }
            return null;
        }
    }
}
