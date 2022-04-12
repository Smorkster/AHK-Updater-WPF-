namespace AHKUpdater.Model
{
    /// <summary> Interface for declaring Data-object </summary>
    public interface IData
    {
        /// <summary> Add a new item </summary>
        /// <param name="item">Item to add</param>
        void Add ( object item );

        /// <summary> Delete the named item </summary>
        /// <param name="name">Name of item to delete</param>
        void Delete ( string name );

        /// <summary> Verifies if an item with the specified name exists </summary>
        /// <param name="name">Name to check for</param>
        /// <returns></returns>
        bool Exists ( string name );
    }
}
