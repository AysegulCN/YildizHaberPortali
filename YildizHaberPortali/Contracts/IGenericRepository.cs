using System.Collections.Generic;
using System.Threading.Tasks;

namespace YildizHaberPortali.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        // ...
        Task<IEnumerable<T>> GetAllAsync(); // <<< Task<ICollection<T>> yerine bunu kullanın
        // ...
    }
}