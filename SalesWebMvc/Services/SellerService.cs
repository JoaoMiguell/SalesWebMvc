using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System.Collections.Generic;
using System.Linq;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList();
        }

        public void Insert(Seller seller)
        {
            _context.Seller.Add(seller);
            _context.SaveChanges();
        }

        public Seller FindById(int id)
        {
            return _context.Seller.Include(obj => obj.Departament).FirstOrDefault(obj => obj.Id == id)!;
        }

        public void Remove(int id)
        {
            Seller seller = _context.Seller.Find(id)!;
            _context.Seller.Remove(seller);
            _context.SaveChanges();
        }

        public void Update(Seller seller)
        {
            if(!_context.Seller.Any(obj => obj.Id == seller.Id))
                throw new NotFoundException("Id not found");

            try
            {
                _context.Update(seller);
                _context.SaveChanges();
            }
            catch(DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
