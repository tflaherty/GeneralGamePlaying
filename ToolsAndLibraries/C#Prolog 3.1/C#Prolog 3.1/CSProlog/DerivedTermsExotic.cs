/*-----------------------------------------------------------------------------------------

  C#Prolog -- Copyright (C) 2007-2013 John Pool -- j.pool@ision.nl

  This library is free software; you can redistribute it and/or modify it under the terms of
  the GNU General Public License as published by the Free Software Foundation; either version
  2 of the License, or any later version.

  This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
  See the GNU General Public License for details, or enter 'license.' at the command prompt.

-------------------------------------------------------------------------------------------*/

using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Linq;

namespace Prolog
{
  public partial class Engine
  {
    #region AltListTerm
    public class AltListTerm : ListTerm
    {
      public override string FunctorToString { get { return functor.ToString ().ToAtom (); } }

      public AltListTerm (string leftBracket, string rightBracket)
      {
        isAltList = true;
        functor = leftBracket + ' ' + rightBracket;
        this.leftBracket = leftBracket;
        this.rightBracket = rightBracket;
      }

      public AltListTerm (string leftBracket, string rightBracket, BaseTerm t0, BaseTerm t1)
        : base (t0.ChainEnd (), t1.ChainEnd ())
      {
        isAltList = true;
        functor = leftBracket + ' ' + rightBracket;
        this.leftBracket = leftBracket;
        this.rightBracket = rightBracket;
      }

      public AltListTerm (string leftBracket, string rightBracket, BaseTerm [] a)
        : base (a)
      {
        isAltList = true;
        functor = leftBracket + ' ' + rightBracket;
        this.leftBracket = leftBracket;
        this.rightBracket = rightBracket;
      }

      public static AltListTerm ListFromArray (
        string leftBracket, string rightBracket, BaseTerm [] ta, BaseTerm afterBar)
      {
        AltListTerm result = null;

        for (int i = ta.Length - 1; i >= 0; i--)
          result = new AltListTerm (leftBracket, rightBracket, ta [i], result == null ? afterBar : result);

        return result;
      }


      public override ListTerm Reverse ()
      {
        AltListTerm result = new AltListTerm (leftBracket, rightBracket);

        foreach (BaseTerm t in this) result =
          new AltListTerm (leftBracket, rightBracket, t, result);

        return result;
      }


      public override ListTerm FlattenList ()
      {
        List<BaseTerm> a = FlattenListEx (functor);

        AltListTerm result = new AltListTerm (leftBracket, rightBracket);

        for (int i = a.Count - 1; i >= 0; i--)
          result = new AltListTerm (leftBracket, rightBracket, a [i], result); // [a0, a0, ...]

        return result;
      }

      List<BaseTerm> FlattenListEx ()
      {
        throw new NotImplementedException ();
      }
    }
    #endregion AltListTerm

    #region IntRangeTerm // to accomodate integer ranges such as X = 1..100, R is X.
    public class IntRangeTerm : CompoundTerm
    {
      BaseTerm lowBound;
      BaseTerm hiBound;
      IEnumerator iEnum;
      public override bool IsCallable { get { return false; } }

      public IntRangeTerm (BaseTerm lowBound, BaseTerm hiBound)
        : base ("..", lowBound, hiBound)
      {
        this.lowBound = lowBound;
        this.hiBound = hiBound;
        iEnum = GetEnumerator ();
      }

      public IntRangeTerm (IntRangeTerm that) // for copying only
        : base ("..", that.lowBound, that.hiBound)
      {
        this.lowBound = that.lowBound;
        this.hiBound = that.hiBound;
        iEnum = GetEnumerator ();
      }

      public ListTerm ToList ()
      {
        ListTerm result = ListTerm.EMPTYLIST;

        int lo = lowBound.To<int> ();
        int hi = hiBound.To<int> ();

        for (int i = hi; i >= lo; i--)
          result = new ListTerm (new DecimalTerm (i), result);

        return result;
      }

      IEnumerator GetEnumerator ()
      {
        int lo = lowBound.To<int> ();
        int hi = hiBound.To<int> ();

        for (int i = lo; i <= hi; i++)
          yield return new DecimalTerm (i);
      }

      public bool GetNextValue (out DecimalTerm dt)
      {
        dt = null;

        if (!iEnum.MoveNext ()) return false;

        dt = (DecimalTerm)iEnum.Current;

        return true;
      }


      public override string ToWriteString (int level)
      {
        return string.Format ("{0}..{1}", lowBound.To<int> (), hiBound.To<int> ());
      }


      public override void TreePrint (int level, Engine e)
      {
        e.WriteLine ("{0}{1}..{2}", Spaces (2 * level), lowBound.To<int> (), hiBound.To<int> ());
      }
    }
    #endregion IntRangeTerm

    #region ComplexTerm // can be used with the is-operator
    public class ComplexTerm : NumericalTerm
    {
      double re;
      double im;
      public double Re { get { return re; } }
      public double Im { get { return im; } }
      double magnitude { get { return Math.Sqrt ((double)(re * re + im * im)); } }
      double arg { get { return Math.Atan2 (im, re); } }
      public double Magnitude { get { return magnitude; } }
      public double Phi { get { return arg; } } // name Arg already in use
      public DecimalTerm ReTerm { get { return new DecimalTerm (re); } }
      public DecimalTerm ImTerm { get { return new DecimalTerm (im); } }
      ComplexTerm eposx;
      ComplexTerm enegx;
      static ComplexTerm ZERO = new ComplexTerm (0f, 0f);
      static ComplexTerm ONE = new ComplexTerm (1f, 0f);
      static ComplexTerm TWO = new ComplexTerm (2f, 0f);
      static ComplexTerm I = new ComplexTerm (0f, 1f);
      static ComplexTerm TWO_I = new ComplexTerm (0f, 2f);
      static ComplexTerm MINUS_I = new ComplexTerm (0f, -1f);
      static ComplexTerm MINUS_2I = new ComplexTerm (0f, -2f);

      public ComplexTerm (decimal re, decimal im)
      {
        this.re = (double)re;
        this.im = (double)im;
        functor = string.Format ("{0}+{1}i", re, im);
        termType = TermType.Number;
      }

      public ComplexTerm (DecimalTerm d)
      {
        this.re = d.ValueD;
        this.im = 0;
        functor = string.Format ("{0}+{1}i", re, im);
        termType = TermType.Number;
      }

      public ComplexTerm (double re, double im)
      {
        this.re = re;
        this.im = im;
        functor = string.Format ("{0}+{1}i", re, im);
        termType = TermType.Number;
      }


      public override int CompareTo (BaseTerm t)
      {
        ComplexTerm c;
        DecimalTerm d;

        if (t is ComplexTerm)
        {
          c = (ComplexTerm)t;

          if (re == c.re && im == c.im) return 0;

          return magnitude.CompareTo (((ComplexTerm)c).magnitude); // compare |this| and |c|
        }

        if (t is DecimalTerm)
        {
          d = (DecimalTerm)t;

          if (im == 0) return re.CompareTo (d.ValueD);

          return magnitude.CompareTo (d.ValueD);
        }

        IO.Error ("Relational operator cannot be applied to '{0}' and '{1}'", this, t);

        return 0;
      }


      public override bool Unify (BaseTerm t, VarStack varStack)
      {
        if (t is Variable) return t.Unify (this, varStack);

        NextUnifyCount ();
        const double eps = 1.0e-6; // arbitrary, cosmetic

        if (t is DecimalTerm)
          return (Math.Abs (im) < eps &&
                   Math.Abs (re - ((DecimalTerm)t).ValueD) < eps);

        if (t is ComplexTerm)
          return (Math.Abs (re - ((ComplexTerm)t).Re) < eps &&
                   Math.Abs (im - ((ComplexTerm)t).Im) < eps);

        //if (t is ComplexTerm)
        //  return ( re == ((ComplexTerm)t).Re && im == ((ComplexTerm)t).Im );

        return false;
      }


      ComplexTerm TimesI ()
      {
        return new ComplexTerm (-im, re);
      }


      ComplexTerm Plus1 ()
      {
        return new ComplexTerm (re + 1, im);
      }


      // sum
      public ComplexTerm Add (ComplexTerm c)
      {
        return new ComplexTerm (re + c.re, im + c.im);
      }

      public ComplexTerm Add (DecimalTerm d)
      {
        return new ComplexTerm (d.ValueD + re, im);
      }

      // difference
      public ComplexTerm Subtract (ComplexTerm c)
      {
        return new ComplexTerm (re - c.re, im - c.im);
      }


      public ComplexTerm Subtract (DecimalTerm d)
      {
        return new ComplexTerm (re - d.ValueD, im);
      }

      // product
      public ComplexTerm Multiply (ComplexTerm c)
      {
        return new ComplexTerm (re * c.re - im * c.im, re * c.im + c.re * im);
      }

      public ComplexTerm Multiply (DecimalTerm d)
      {
        return new ComplexTerm (re * d.ValueD, im * d.ValueD);
      }

      // quotient
      public ComplexTerm Divide (ComplexTerm c)
      {
        if (c.re == 0 && c.im == 0)
          IO.Error ("Division by zero complex number not allowed");

        double denominator = c.re * c.re + c.im * c.im;
        double newRe = (re * c.re + im * c.im) / denominator;
        double newIm = (im * c.re - re * c.im) / denominator;

        return new ComplexTerm (newRe, newIm);
      }


      public ComplexTerm Negative ()
      {
        return new ComplexTerm (-re, -im);
      }


      public ComplexTerm Conjugate ()
      {
        return new ComplexTerm (re, -im);
      }


      public ComplexTerm Divide (DecimalTerm d)
      {
        if (d.Value == 0)
          IO.Error ("Division by zero not allowed");

        return new ComplexTerm (re / d.ValueD, im / d.ValueD);
      }


      public ComplexTerm Log () // log|z| + i.arg(z), calculate the principal value
      {
        return new ComplexTerm (Math.Log (magnitude), arg);
      }


      public ComplexTerm Sqrt ()
      {
        return new ComplexTerm (Math.Sqrt ((magnitude + re) / 2), Math.Sqrt ((magnitude - re) / 2));
      }


      public ComplexTerm Sqr ()
      {
        return new ComplexTerm (re * re - im * im, 2 * re * im);
      }


      public ComplexTerm Exp () // engine^re * (cos (im) + i sin (im))
      {
        double exp = Math.Exp (re);

        return new ComplexTerm (exp * Math.Cos (im), exp * Math.Sin (im));
      }


      public ComplexTerm Abs ()
      {
        return new ComplexTerm (magnitude, 0);
      }


      public ComplexTerm Power (DecimalTerm d) // z^n = r^n * (cos (n*phi) + i*sin(n*phi))
      {
        if (d.IsInteger)
        {
          int n = d.To<int> ();

          if (re == 0 && im == 0)  // try Google 'what is 0 to the power of 0', etc.
            return new ComplexTerm (n == 0 ? 1 : 0, 0f);

          double rn = Math.Pow (magnitude, n);
          double nArg = n * arg;

          return new ComplexTerm (rn * Math.Cos (nArg), rn * Math.Sin (nArg));
        }
        else
          return (Log ().Multiply (d)).Exp (); // t^d = exp(log(t^d)) = exp(d*log(t))
      }

      // http://en.wikipedia.org/wiki/Exponentiation#Failure_of_power_and_logarithm_identities
      public ComplexTerm Power (ComplexTerm p) // also see IEEE 754-2008 floating point standard
      {
        if (re == 0 && im == 0)
          return new ComplexTerm ((p.re == 0 && p.im == 0 ? 1 : 0), 0f);

        return (Log ().Multiply (p)).Exp (); // t^p = exp(log(t^p)) = exp(p*log(t))
      }


      public ComplexTerm Sin () // (exp(iz) - exp(-iz)) / 2i
      {
        eposx = TimesI ().Exp ();
        enegx = (TimesI ().Negative ()).Exp ();

        return (eposx.Subtract (enegx)).Divide (TWO_I);
      }


      public ComplexTerm Cos () // (exp(iz) + exp(-iz)) / 2
      {
        eposx = TimesI ().Exp ();
        enegx = (TimesI ().Negative ()).Exp ();

        return (eposx.Add (enegx)).Divide (TWO);
      }


      public ComplexTerm Tan () //
      {
        eposx = TimesI ().Exp ();
        enegx = (TimesI ().Negative ()).Exp ();

        return (eposx.Subtract (enegx)).Divide ((eposx.Add (enegx)));
      }


      public ComplexTerm Sinh ()
      {
        eposx = Exp ();             // engine^z
        enegx = Negative ().Exp (); // engine^(-z)

        return (eposx.Subtract (enegx)).Divide (TWO);
      }


      public ComplexTerm Cosh ()
      {
        eposx = Exp ();             // engine^z
        enegx = Negative ().Exp (); // engine^(-z)

        return (eposx.Add (enegx)).Divide (TWO);
      }


      public ComplexTerm Tanh ()
      {
        eposx = Exp ();             // engine^z
        enegx = Negative ().Exp (); // engine^(-z)

        return (eposx.Subtract (enegx)).Divide (eposx.Add (enegx));
      }


      public ComplexTerm Asin () // log(z + sqrt(1+z^2))
      {
        return Add ((Sqr ().Plus1 ()).Sqrt ()).Log ();
      }


      public ComplexTerm Acos () // 2 * log( sqrt((z+1)/2) + sqrt((z-1)/2))
      {
        return ComplexTerm.ZERO;
      }


      public ComplexTerm Atan () // (log(1+z) - log(1-z)) / 2
      {
        ComplexTerm c0 = Plus1 ().Log ();
        ComplexTerm c1 = (Negative ().Plus1 ()).Log ();

        return (c0.Subtract (c1)).Divide (TWO);
      }


      public override string ToWriteString (int level)
      {
        // rounding: arbitrary, cosmetic, in order to prevent answers like 0+i or 1+0i
        double re = Math.Round (this.re, 6);
        double im = Math.Round (this.im, 6);

        if (im == 0)
          return (re == 0 ? "0" : re.ToString ("0.######", Utils.CIC));
        else // im != 0
        {
          string ims = null;

          if (im == -1)
            ims = "-";
          else if (im != 1)
            ims = im.ToString ("0.######", Utils.CIC);

          if (re == 0)
            return string.Format (Utils.CIC, "{0}i", ims);
          else
            return string.Format (Utils.CIC, "{0:0.######}{1}{2}i", re, (im < 0 ? null : "+"), ims);
        }
      }
    }
    #endregion ComplexTerm

  }
}
