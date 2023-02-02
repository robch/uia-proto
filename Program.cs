using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using UIAutomationClient;
using Windows.Win32.UI.Accessibility;

public class Program
{
    static void Main(string[] args)
    {
        var program = new Program();
        program.Run();
    }

    public Program()
    {
        _automation = new CUIAutomation();
        _root = _automation.GetRootElement();
        _walker = _automation.ControlViewWalker;
    }

    private void Run()
    {
        for (var time = 3; time > 0; time--)
        {
            Console.Write($"\r{time}...   ");
            Thread.Sleep(1000);
        }
        Console.WriteLine("\rGo!!!   ");

        var stopWatch = new Stopwatch();
        stopWatch.Start();

        var focus = _automation.GetFocusedElement();
        var top = GetTopElement(focus);

        Console.WriteLine(top.CurrentName);
        Console.WriteLine("---");
        var found = FindAllElementsUnder(top);

        stopWatch.Stop();
        Console.WriteLine($"Ellapsed time: {stopWatch.Elapsed}");

        found.Sort((a, b) => CompareElements(a, b));
        foreach (var item in found)
        {
            var name = item.Item1;
            var rect = item.Item2;

            var x1 = rect.left;
            var y1 = rect.top;
            var x2 = rect.right;
            var y2 = rect.bottom;
            Console.WriteLine($"[{x1},{y1},{x2},{y2}] '{name}'");
        }
    }

    private int CompareElements(Tuple<string, tagRECT> a, Tuple<string, tagRECT> b)
    {
        var r1 = a.Item2;
        var r2 = b.Item2;
        if (r1.top < r2.top) return -1;
        if (r1.top > r2.top) return 1;
        if (r1.left < r2.left) return -1;
        if (r1.left > r2.left) return 1;
        if (r1.right < r2.right) return -1;
        if (r1.right > r2.right) return 1;
        if (r1.bottom < r2.bottom) return -1;
        if (r1.bottom > r2.bottom) return 1;
        return 0;
    }

    IUIAutomationElement GetTopElement(IUIAutomationElement node)
    {
        if (node == _root) return node;

        while (true)
        {
            var parent = _walker.GetParentElement(node);

            var compare = _automation.CompareElements(parent, _root);
            if (compare != 0) break;

            node = parent;
        }

        return node;
    }

    private List<Tuple<string,tagRECT>> FindAllElementsUnder(IUIAutomationElement node)
    {
        var elements = new List<Tuple<string, tagRECT>>();
        FindAllElementsUnder(node, ref elements);
        return elements;
    }

    private void FindAllElementsUnder(IUIAutomationElement node, ref List<Tuple<string, tagRECT>> elements)
    {
        var condition = _automation.CreatePropertyCondition((int)UIA_PROPERTY_ID.UIA_IsEnabledPropertyId, true);
        var walker = _automation.CreateTreeWalker(condition);

        for (var child = walker.GetFirstChildElement(node); child != null; child = walker.GetNextSiblingElement(child))
        {
            elements.Add(new Tuple<string, tagRECT>(child.CurrentName, child.CurrentBoundingRectangle));
            FindAllElementsUnder(child, ref elements);
        }
    }

    IUIAutomation _automation;
    IUIAutomationElement _root;
    IUIAutomationTreeWalker _walker;
}



