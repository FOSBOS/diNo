using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
  // alle Repository-Objekte müssen dieses Interface implementieren. Es liefert die ID des Objekts
  public interface IRepositoryObject
  {
    int GetID();    
  }
  
  // Schüler, Kurse, Klassen, ... sollen effizient verwaltet werden, 
  // indem sie nur einmal aus der DB geladen werden
  public class Repository<T> where T : IRepositoryObject
  {
    public Dictionary<int, T> Liste;
    public delegate T CreateObj(int id); // Methode, die ein Objekt vom Typ T mit dieser ID erzeugt
    CreateObj konstruktorT;              // entspricht z.B.  new Schueler(id)

    public Repository(CreateObj creator)
    {
      Liste = new Dictionary<int, T>();
      konstruktorT=creator;
    }
    
    public void Add(T obj)
    {
      Liste.Add(obj.GetID(),obj);
    }

    public T Find(int id)
    {
      T res;
      if (Liste.TryGetValue(id, out res))
      {
        return res;
      }
      else
      {
        // gibt es diese id nicht, so wird dieses Objekt angelegt
        res = konstruktorT(id);
        Add(res);
        return res;        
      }
        
    }
  }
}
