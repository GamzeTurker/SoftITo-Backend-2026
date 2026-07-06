using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EduCore.Data.Repository.IRepository
{
    public interface IRepository<T>where T : class //sadece class alabilir
    {
        void Add(T entity);
        void Update(T entity);

        void Remove(T entity);
        //tek satır veri çekmek için kullanırız amacı eager loading ile birden fazla sql sorgularını direkt olarak
        //çekmek için  kullanılır include parametresi ise ilişkili tablolarda veri varsa getirsin yoksa null alsın
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? icludeProperties = null);

        //burda ise direk listeleme yapcaz sql sorgularını  hızlı birşekişlde geitrnesi için expression func kullandık  burda tüm veriler gelicek
        //eger gerekli ise ilişkili tablolarda gelir gelmezse null gelir ama arka planda sql sorguları yüklenir
        //müsteri tablosu full listele gibi 
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        void RemoveRange(IEnumerable<T> entities);
        //bu metodu yazmamızın sebebi ilişkiliş tablolardaki alanlarıda rahatlıkla silebilmek amacı ile yaptık 
    }
}
