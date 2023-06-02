using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Storage.classes
{
    public static class plControl
    {
        public static List<Place> findByCondition(List<Condition> a)
        {
            bool condApro = false;
            List<Place> places = new List<Place>();
            stockEntities lCont = stockEntities.GetStockEntityD();

            foreach (Place pl in lCont.Place.ToList())
            {
                foreach (Condition cnd in a )
                {
                    if(!pl.PlaceCond.Select(p=>p.ID_Condition).Contains(cnd.IDCondition)) condApro = false;
                }
                if (condApro) places.Add(pl);
            }
            return places;
        }

    }
}
