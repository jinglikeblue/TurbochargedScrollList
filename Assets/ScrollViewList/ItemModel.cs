namespace Jing.ScrollViewList
{
    class ItemModel<TData>
    {
        public TData data;
        public float height;
        public float width;

        public ItemModel(TData data)
        {
            this.data = data;
        }
    }
}
