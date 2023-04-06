using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartamentService _departamentService;

        public SellersController(SellerService sellerService, DepartamentService departamentService)
        {
            _sellerService = sellerService;
            _departamentService = departamentService;
        }

        public async Task<IActionResult> Index()
        {
            List<Seller> list = await _sellerService.FindAllAsync();

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            List<Departament> departaments = await _departamentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Departaments = departaments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if(!ModelState.IsValid)
            {
                List<Departament> departaments = await _departamentService.FindAllAsync();
                SellerFormViewModel viewmodel = new() { Seller = seller, Departaments= departaments };
                return View(viewmodel);
            }

            await _sellerService.InsertAsync(seller);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            Seller seller = await _sellerService.FindByIdAsync((int)id);
            if(seller == null)
                return RedirectToAction(nameof(Error), new { message = "Id not found" });

            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sellerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            Seller seller = await _sellerService.FindByIdAsync((int)id);
            if(seller == null)
                return RedirectToAction(nameof(Error), new { message = "Id not found" });

            return View(seller);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            Seller seller = await _sellerService.FindByIdAsync((int)id);
            if(seller == null)
                return RedirectToAction(nameof(Error), new { message = "Id not found" });

            List<Departament> departaments = await _departamentService.FindAllAsync();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = seller, Departaments = departaments };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            if(id != seller.Id)
                return RedirectToAction(nameof(Error), new { message = "Id mismatch" });

            if(!ModelState.IsValid)
            {
                List<Departament> departaments = await _departamentService.FindAllAsync();
                SellerFormViewModel viewmodel = new() { Seller = seller, Departaments = departaments };
                return View(viewmodel);
            }

            try
            {
                await _sellerService.InsertAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch(ApplicationException err)
            {
                return RedirectToAction(nameof(Error), new { message = err.Message });
            }
        }

        public IActionResult Error(string message)
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(errorViewModel);
        }
    }
}
