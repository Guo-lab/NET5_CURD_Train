using System.Linq;
using SharpArch.NHibernate;

namespace ProjectBase.Data
{
    /// <summary>
    ///  ISessionFactoryKeyProvider：缺省实现支持两种方式，一种是类（如DAO类）上加SessionFactory标记，一种是无标记。
    ///  除Transaction外其它各处都通过SessionFactoryKeyProvider来判断使用哪个SessionFactory。
    ///  考虑对于所有Transaction都使用同一SessionFactory，因此要覆写缺省实现来处理无标记情况，缺省实现是无标记就认为是默认数据库，
    ///  覆写逻辑为无标记则先使用已创建的session，无session才新建默认数据库的session。
    /// </summary>
    public class RWSeparateSessionFactoryKeyProvider : ISessionFactoryKeyProvider
    {
        public string GetKey()
        {
            return NHibernateSession.Storage.GetSingleActiveKey()?? NHibernateSession.DefaultFactoryKey; ;
        }

        /// <summary>
        /// Gets the session factory key.
        /// </summary>
        /// <param name="anObject">An object that may have the <see cref="SessionFactoryAttribute"/> applied.</param>
        /// <returns></returns>
        public string GetKeyFrom(object anObject)
        {
            var key= SessionFactoryAttribute.GetKeyFrom(anObject);
            if (key.Equals(NHibernateSession.DefaultFactoryKey))
            {
                key = GetKey();
            }
            return key;
        }
    }
}