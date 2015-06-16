
namespace ExprTranslator.Query
{
    public class OracleTypeSystem : QueryTypeSystem
    {
        public override int StringDefaultSize
        {
            get { return 2000; }
        }
    }
}
