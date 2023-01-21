//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2005  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;


namespace ZedGraph
{
	/// <summary>
	/// The LogScale class inherits from the <see cref="Scale" /> class, and implements
	/// the features specific to <see cref="AxisType.Log" />.
	/// </summary>
	/// <remarks>
	/// LogScale is a non-linear axis in which the values are scaled using the base 10
	/// <see cref="Math.Log(double)" />
	/// function.
	/// </remarks>
	/// 
	/// <author> John Champion  </author>
	/// <version> $Revision: 1.12 $ $Date: 2007-04-16 00:03:02 $ </version>
	[Serializable]
	class LogScale : Scale, ISerializable //, ICloneable
	{

	#region constructors

		/// <summary>
		/// Default constructor that defines the owner <see cref="Axis" />
		/// (containing object) for this new object.
		/// </summary>
		/// <param name="owner">The owner, or containing object, of this instance</param>
		public LogScale( Axis owner )
			: base( owner )
		{
		}

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="LogScale" /> object from which to copy</param>
		/// <param name="owner">The <see cref="Axis" /> object that will own the
		/// new instance of <see cref="LogScale" /></param>
		public LogScale( Scale rhs, Axis owner )
			: base( rhs, owner )
		{
		}

		/// <summary>
		/// Create a new clone of the current item, with a new owner assignment
		/// </summary>
		/// <param name="owner">The new <see cref="Axis" /> instance that will be
		/// the owner of the new Scale</param>
		/// <returns>A new <see cref="Scale" /> clone.</returns>
		public override Scale Clone( Axis owner )
		{
			return new LogScale( this, owner );
		}

	#endregion

	#region properties

		/// <summary>
		/// Return the <see cref="AxisType" /> for this <see cref="Scale" />, which is
		/// <see cref="AxisType.Log" />.
		/// </summary>
		public override AxisType Type
		{
			get { return AxisType.Log; }
		}

		/// <summary>
		/// Gets or sets the minimum value for this scale.
		/// </summary>
		/// <remarks>
		/// The set property is specifically adapted for <see cref="AxisType.Log" /> scales,
		/// in that it automatically limits the setting to values greater than zero.
		/// </remarks>
		public override double Min
		{
			get { return _min; }
			set { if ( value > 0 ) _min = value; }
		}

		/// <summary>
		/// Gets or sets the maximum value for this scale.
		/// </summary>
		/// <remarks>
		/// The set property is specifically adapted for <see cref="AxisType.Log" /> scales,
		/// in that it automatically limits the setting to values greater than zero.
		/// <see cref="XDate" /> struct.
		/// </remarks>
		public override double Max
		{
			get { return _max; }
			set { if ( value > 0 ) _max = value; }
		}

	#endregion

	#region methods

		override public double Linearize( double val )
		{
			return SafeLog( val );
		}

		override public double DeLinearize( double val )
		{
			return Math.Pow( 10.0, val );
		}

        override internal double CalcBaseTic()
        {
            if (FirstDigit == 1)
                return SafeLog(2 * (Math.Pow(10.0, Math.Floor(SafeLog(_max-_min)) - 1)));
            else if (FirstDigit == 2 || FirstDigit == 3 || FirstDigit == 4)
                return SafeLog(5 * (Math.Pow(10.0, Math.Floor(SafeLog(_max-_min)) - 1)));
            else
                return Math.Floor(SafeLog(_max-_min));
        }

        private int FirstDigit
        {
            get { return (int)((_max-_min) / (Math.Pow(10.0, Math.Floor(SafeLog(_max-_min))))); }
        }

        private double MajorStep_Log
        {
            get {return Math.Pow(10.0, CalcBaseTic());}
        }

        override internal int CalcNumTics()
        {
            return 20 * (int)(_max / (_max - _min));
        }

        override internal double CalcMajorTicValue(double baseVal, double tic)
        {
            return SafeLog(Math.Pow(10.0, baseVal) + MajorStep_Log * tic);
        }

        override internal double CalcMinorTicValue(double baseVal, int iTic)
        {
            return SafeLog(Math.Pow(10.0, baseVal) + MajorStep_Log / 5 * iTic);
        }

        override internal int CalcMinorStart(double baseVal)
        {
            return -4;
        }

        override internal string MakeLabel(GraphPane pane, int index, double dVal)
        {
            if (_isUseTenPower)
            {
                if (dVal >= 15)
                {
                    return string.Format("{0:0.######}", Math.Pow(10, dVal - 15)) + " (10^15)";
                }
                else if (dVal >= 12)
                {
                    return string.Format("{0:0.######}", Math.Pow(10, dVal - 12)) + " tn";
                }
                else if (dVal >= 9)
                {
                    return string.Format("{0:0.######}", Math.Pow(10, dVal - 9)) + " bn";
                }
                else if (dVal >= 6)
                {
                    return string.Format("{0:0.######}", Math.Pow(10, dVal - 6)) + " mn";
                }
                else
                {
                    return string.Format("{0:0.######}", Math.Pow(10, dVal));
                }
            }
            else
            {
                return string.Format("{0:0.######}", Math.Pow(10, dVal));
            }
        }

        override public void PickScale(GraphPane pane, Graphics g, float scaleFactor)
        {
            //this method needs to be overriden as empty for log scale
        }


	#endregion

	#region Serialization
		/// <summary>
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		public const int schema2 = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected LogScale( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema2" );

		}
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>

		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			info.AddValue( "schema2", schema2 );
		}
	#endregion

	}
}
