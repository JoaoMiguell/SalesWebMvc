using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using System.Collections.Generic;
using System.Linq;
using SalesWebMvc.Services.Exceptions;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync();
        }

        public async Task InsertAsync(Seller seller)
        {
            _context.Seller.Add(seller);
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Seller.Include(obj => obj.Departament).FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            Seller seller = await _context.Seller.FindAsync(id)!;
            _context.Seller.Remove(seller);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seller seller)
        {
            if(!await _context.Seller.AnyAsync(obj => obj.Id == seller.Id))
                throw new NotFoundException("Id not found");

            try
            {
                _context.Update(seller);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException err)
            {
                throw new DbConcurrencyException(err.Message);
            }
        }
    }
}
