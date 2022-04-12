using System;
using System.ComponentModel;

namespace AHKUpdater.Model
{
    public interface IType
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Guid Id { get; }

        bool IsNew { get; set; }

        string Name { get; set; }

        string Value { get; set; }
    }
}
