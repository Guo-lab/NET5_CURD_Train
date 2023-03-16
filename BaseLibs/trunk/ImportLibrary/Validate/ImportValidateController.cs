using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Application;

namespace ImportLibrary.Validate
{
    public class ImportValidateController<T> : IImportValidateController<T> where T : BaseImportItem
    {
        protected IImportValidateHandler<T> _handler;
        public IEnumerable<T> ImportItems
        {
            get; set;
        }

        public ImportValidateController()
        {
            
            _handler = CreateHandlerChain();
        }

        private IImportValidateHandler<T> CreateHandlerChain()
        {
            var handlers = CastleContainer.WindsorContainer.ResolveAll<IImportValidateHandler<T>>();
            if (handlers.Length > 0)
            {
                var sortedHandler = handlers.OrderBy(x =>
                {
                    OrderAttribute orderAttribute = (OrderAttribute)Attribute.GetCustomAttribute(x.GetType(), typeof(OrderAttribute));
                    if (orderAttribute == null)
                    {
                        return 100;
                    }
                    else
                    {
                        return orderAttribute.ValidateOrder;
                    }
                }).ToList();

                var handler = sortedHandler[0];
                for(int i = 1; i < sortedHandler.Count; i++)
                {
                    handler.SetNextHandler(sortedHandler[i]);
                    handler = sortedHandler[i];
                }
                return sortedHandler[0];
            }
            else
            {
                return null;
            }
        }

        public void StartValidate()
        {
            if (_handler != null)
            {
                _handler.HandleValidate(this.ImportItems);
            }
        }
    }
}
