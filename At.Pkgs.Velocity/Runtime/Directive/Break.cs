using System;
using System.Collections.Generic;
using System.Text;
using At.Pkgs.Velocity.Context;
using System.IO;
using At.Pkgs.Velocity.Runtime.Parser.Node;

namespace At.Pkgs.Velocity.Runtime.Directive
{
    public class Break : Directive
    {

        public override string Name
        {
            get
            {
                return "break";
            }
            set { throw new NotSupportedException(); }
        }

        /**
         * Return type of this directive.
         * @return The type of this directive.
         */
        public override DirectiveType Type
        {
            get
            {
                return DirectiveType.LINE;
            }
        }

        public override bool AcceptParams
        {
            get
            {
                return false;
            }
        }


        /**
         * Break directive does not actually do any rendering. 
         * 
         * This directive throws a BreakException (RuntimeException) which
         * signals foreach directive to break out of the loop. Note that this
         * directive does not verify that it is being called inside a foreach
         * loop.
         * 
         * @param context
         * @param writer
         * @param node
         * @return true if the directive rendered successfully.
         * @throws IOException
         * @throws MethodInvocationException
         * @throws ResourceNotFoundException
         * @throws ParseErrorException
         */
        public override bool Render(IInternalContextAdapter context, TextWriter writer, At.Pkgs.Velocity.Runtime.Parser.Node.INode node)
        {
            throw new BreakException();
        }
    }

    public class BreakException : System.Exception
    {

    }
}
