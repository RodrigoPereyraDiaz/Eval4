namespace Eval4
{
    public class SyntaxError 
    {
        public readonly string message;
        public readonly string formula;
        public readonly int pos;
        
        public SyntaxError(string message, string formula, int pos)
        {
            this.message = message;
            this.formula = formula;
            this.pos = pos;
        }
    }
}
