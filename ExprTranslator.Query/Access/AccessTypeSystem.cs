
namespace ExprTranslator.Query
{
    public class AccessTypeSystem : QueryTypeSystem
    {
        public override int StringDefaultSize
        {
            get { return 2000; }
        }

        public override int BinaryDefaultSize
        {
            get { return 4000; }
        }
    }
}
