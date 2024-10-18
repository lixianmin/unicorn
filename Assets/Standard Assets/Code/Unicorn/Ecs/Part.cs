
/********************************************************************
created:    2018-03-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public class Part : IDisposable, IHaveEntity, IInitPart, IPart, IIsDisposed
    {
        void IInitPart.InitPart(EntityBase entity)
        {
            _entity = entity;
            Awake();
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            OnDestroy();
        }

        public EntityBase GetEntity()
        {
            return _entity;
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        protected virtual void Awake() { }
        protected virtual void OnDestroy() { }

        private EntityBase _entity;
        private bool _isDisposed;
    }
}