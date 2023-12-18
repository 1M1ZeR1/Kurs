using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kurs
{
    class FavoritesList:ProccessesForReport
    {
        List<ProccessesForReport> listOfFavoritesProcceses = new List<ProccessesForReport>();

        public void addingProccess(string proccess,int Id)
        {
            listOfFavoritesProcceses.Add(new ProccessesForReport());
            listOfFavoritesProcceses[listOfFavoritesProcceses.Count-1].proccessName = proccess;
            listOfFavoritesProcceses[listOfFavoritesProcceses.Count-1].Id = Id;
        }
        public void addingInfo(OtherProccess proccess,double CPU,double Memory)
        {
            for(int i=0;i<listOfFavoritesProcceses.Count;i++)
            {
                if(proccess.Id == listOfFavoritesProcceses[i].Id)
                {
                    listOfFavoritesProcceses[i].proccessCpu += CPU;
                    listOfFavoritesProcceses[i].proccessMemory += Memory;
                    listOfFavoritesProcceses[i].currentCorent++;
                    break;
                }
            }
        }
        public List<ProccessesForReport> finishList()
        {
            for(int i=0;i < listOfFavoritesProcceses.Count; i++)
            {
                listOfFavoritesProcceses[i].proccessCpu = listOfFavoritesProcceses[i].proccessCpu / listOfFavoritesProcceses[i].currentCorent;
                listOfFavoritesProcceses[i].proccessMemory = listOfFavoritesProcceses[i].proccessMemory / listOfFavoritesProcceses[i].currentCorent;
            }
            return listOfFavoritesProcceses;
        }
    }
}
