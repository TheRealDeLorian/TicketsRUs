using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryTRU.Data;

namespace LibraryTRU.IServices
{
    public interface IConcertService
    {
        public Task<IEnumerable<Concert>> GetAll();
        public Task AddNewAsync(Concert concert);
        //public Task<Concert> Add(string email, int concertId);
    }
}
