using System.Collections.Generic;

namespace diNo
{
  // alle Repository-Objekte müssen dieses Interface implementieren. Es liefert die ID des Objekts
  public interface IRepositoryObject
  {
    int GetId();
    string Comparer(); // diese Methode liefert ein im Objekt festgelegtes Attribut um es mit einem anderen zu vgl. (z.B. Kürzel bei Lehrer)
  }


  // Schüler, Kurse, Klassen, ... sollen effizient verwaltet werden, 
  // indem sie nur einmal aus der DB geladen werden
  public class Repository<T> where T : IRepositoryObject
  {
    private Dictionary<int, T> Liste;
    public delegate T CreateObj(int id); // Methode, die ein Objekt vom Typ T mit dieser ID erzeugt
    CreateObj konstruktorT;              // entspricht z.B.  new Schueler(id)

    public Repository(CreateObj creator)
    {
      Liste = new Dictionary<int, T>();
      konstruktorT = creator;
    }

    public void Add(T obj)
    {
      try
      {
        Liste.Add(obj.GetId(), obj);
      }
      catch
      {
        ; // wenn das Objekt schon drin ist, ist es auch egal
      }
    }

    // sucht, ob es ein Objekt mit dieser ID gibt und legt es sonst an
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

    // sucht, ob es ein Objekt mit dem zugehörigen Kürzel gibt (wird im Objekt festgelegt)
    public T Find(string kuerzel)
    {
      foreach (T t in Liste.Values)
      {
        if (t.Comparer() == kuerzel)
          return t;
      }
      return default(T);
    }

    public bool Contains(int id)
    {
      return Liste.ContainsKey(id);
    }

    public void Clear()
    {
      Liste.Clear();
    }

    public void Remove(int id)
    {
      Liste.Remove(id);
    }

    public List<T> getList()
    {
      var q = new List<T>();
      foreach (var e in Liste.Values)
        q.Add(e);
      return q;
    }
  }
}
