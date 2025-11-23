using Lab_01.Models;
using Lab_01.Services;
using Lab_01.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Lab01.Mvc.Controllers;

public class ListsController : Controller
{
    private readonly IListService _service;

    public ListsController(IListService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index() => View(new ListViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Transform(ListViewModel vm)
    {
        if (!ModelState.IsValid) return View("Index", vm);

        var original = MyList.CreateList(vm.Input);
        var zeros = _service.CountZeros(original);
        var transformed = _service.ListModification(original);

        vm.ParsedAs = original.ToString();
        vm.ZeroCount = zeros;
        vm.ResultAs = transformed.ToString();

        vm.HeadValue = null;
        vm.TailAs = null;

        return View("Index", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Head(ListViewModel vm)
    {
        if(!ModelState.IsValid) return View("Index", vm);

        try
        {
            var original = MyList.CreateList(vm.Input);

            vm.ParsedAs = original.ToString();
            vm.HeadValue = _service.SelectHead(original)?.ToString();

            vm.ZeroCount = null;
            vm.ResultAs = null;
            vm.TailAs = null;

            return View("Index", vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Tail(ListViewModel vm)
    {
        if (!ModelState.IsValid) return View("Index", vm);

        try
        {
            var original = MyList.CreateList(vm.Input);

            vm.ParsedAs = original.ToString();
            vm.TailAs = _service.SelectTail(original).ToString();

            vm.ZeroCount = null;
            vm.ResultAs = null;
            vm.HeadValue = null;

            return View("Index", vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", vm);
        }
    }
}
