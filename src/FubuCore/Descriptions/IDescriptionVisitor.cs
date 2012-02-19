namespace FubuCore.Descriptions
{
    public interface IDescriptionVisitor
    {
        void Start(Description description);
        void StartList(BulletList list);
        void EndList();
        void End();
    }
}