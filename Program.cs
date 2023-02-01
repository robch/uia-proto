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
        WalkEnabledElements(top);

        stopWatch.Stop();
        Console.WriteLine($"Ellapsed time: {stopWatch.Elapsed}");
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

    private void WalkEnabledElements(IUIAutomationElement node, int level = 0)
    {
        // UIA_CONTROLTYPE_ID.UIA_DocumentControlTypeId;

        //var condition1 = _automation.CreatePropertyCondition((int)UIA_PROPERTY_ID.UIA_IsControlElementPropertyId, true);
        var condition2 = _automation.CreatePropertyCondition((int)UIA_PROPERTY_ID.UIA_IsEnabledPropertyId, true);
        //var andCondition = _automation.CreateAndCondition(condition1, condition2);
        var walker = _automation.CreateTreeWalker(condition2);

        var indent = new string(' ', level * 2);
        for (var child = walker.GetFirstChildElement(node); child != null; child = walker.GetNextSiblingElement(child))
        {
            Console.WriteLine($"{indent} {child.CurrentName}\n{indent} (type={child.CurrentControlType})");
            WalkEnabledElements(child, level + 1);
        }
    }

    IUIAutomation _automation;
    IUIAutomationElement _root;
    IUIAutomationTreeWalker _walker;
}



