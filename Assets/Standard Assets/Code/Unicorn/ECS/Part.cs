
/********************************************************************
created:    2018-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public abstract class Part : IDisposable, IHaveEntity, IInitPart, IPart, IIsDisposed
    {
        void IInitPart.InitPart(Entity entity)
		{
			_entity = entity;
			_DoInitPart();
		}

        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _DoDispose();
        }

        public Entity GetEntity()
        {
            return _entity;
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        protected virtual void _DoInitPart() { }
        protected virtual void _DoDispose() { }

        private Entity _entity;
        private bool _isDisposed;
    }
}