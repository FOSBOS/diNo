using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diNo
{
	public enum Schulaufgabenwertung
	{
    /// <summary>
    /// Nicht gesetzt.
    /// </summary>
    NotSet = -1,
		/// <summary>
		/// Keine Schulaufgaben, sondern Kurzarbeiten und Exen.
		/// </summary>
		KurzarbeitenUndExen = 0,
		/// <summary>
		/// Wertung Eins zu Eins.
		/// </summary>
		EinsZuEins = 1,
		/// <summary>
		/// Wertung Zwei zu Eins.
		/// </summary>
		ZweiZuEins = 2
	}
}
