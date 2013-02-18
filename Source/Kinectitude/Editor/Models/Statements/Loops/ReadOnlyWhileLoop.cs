
namespace Kinectitude.Editor.Models.Statements.Loops
{
    internal sealed class ReadOnlyWhileLoop : AbstractWhileLoop
    {
        private readonly AbstractWhileLoop inheritedLoop;

        public override string Expression
        {
            get { return inheritedLoop.Expression; }
            set { }
        }

        public ReadOnlyWhileLoop(AbstractWhileLoop inheritedLoop) : base(inheritedLoop)
        {
            this.inheritedLoop = inheritedLoop;
        }

        public override void Accept(Storage.IGameVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
