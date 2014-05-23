/*-----------------------------------------------------------------------------------------

  C#Prolog -- Copyright (C) 2007-2013 John Pool -- j.pool@ision.nl

  This library is free software; you can redistribute it and/or modify it under the terms of
  the GNU General Public License as published by the Free Software Foundation; either version
  2 of the License, or any later version.

  This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
  See the GNU General Public License for details, or enter 'license.' at the command prompt.

-------------------------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace Prolog
{
  public enum DupMode { DupIgnore, DupAccept, DupError };

  public partial class Engine
  {
    public class BaseTermSet : List<BaseTerm>
    {
      DupMode dupMode;

      public BaseTermSet ()
      {
        dupMode = DupMode.DupAccept;
      }


      public BaseTermSet (DupMode dm)
      {
        dupMode = dm;
      }


      public BaseTermSet (BaseTerm list)
      {
        while (list.Arity == 2)
        {
          Add (list.Arg (0));
          list = list.Arg (1);
        }
      }


      public void Insert (BaseTerm termToInsert)
      {
        int i = BinarySearch (termToInsert);

        if (i >= 0) // found
        {
          if (dupMode == DupMode.DupAccept) Insert (i, termToInsert);
        }
        else
          Insert (~i, termToInsert);
      }


      public ListTerm ToList ()
      {
        ListTerm t = ListTerm.EMPTYLIST;

        for (int i = Count - 1; i >= 0; i--)
          t = new ListTerm (this [i], t); // [a0, a0, ...]

        return t;
      }
    }
  }
}
