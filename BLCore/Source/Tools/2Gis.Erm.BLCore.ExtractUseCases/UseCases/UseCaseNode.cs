using System;

using Roslyn.Compilers.Common;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.UseCases
{
    public class UseCaseNode
    {
        private readonly int _level;

        public UseCaseNode(int level)
        {
            _level = level;
        }

        public INamedTypeSymbol ContainingClass { get; set; }
        public INamedTypeSymbol Request { get; set; }
        public string RequestKey { get; set; }
        public UseCaseNode[] ChildNodes { get; set; }

        public int Level
        {
            get
            {
                return _level;
            }
        }

        public override string ToString()
        {
            return string.Format("Level:{0}. Type:{1}.{2}Request:{3}", Level, ContainingClass, Environment.NewLine, Request);
        }
    }
}