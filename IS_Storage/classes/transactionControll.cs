using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Storage.classes
{
    public static class transactionControll
    {
        public static DbSet<Product> ProdofClients(Client searchClient)
        {
            return stockEntities.GetStockEntity().Product;
        }
    }
}
