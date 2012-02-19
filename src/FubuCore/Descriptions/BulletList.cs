using System.Collections.Generic;

namespace FubuCore.Descriptions
{
    public class BulletList
    {
        public BulletList()
        {
            Children = new List<Description>();
        }

        public IList<Description> Children { get; private set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsOrderDependent { get; set; }

        public void AcceptVisitor(IDescriptionVisitor visitor)
        {
            visitor.StartList(this);

            Children.Each(x => x.AcceptVisitor(visitor));

            visitor.EndList();
        }
    }
}