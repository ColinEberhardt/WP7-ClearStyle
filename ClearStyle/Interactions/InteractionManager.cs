using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using ClearStyle.ViewModel;

namespace ClearStyle.Interactions
{
	/// <summary>
	/// Manages a collection of interactions, multicasting various functions to each interaction (such
	/// as the need to attached to a new element), and also manages the enabled state of each interaction.
	/// </summary>
	public class InteractionManager
	{
		private List<IInteraction> _interactions = new List<IInteraction>();

		public void AddInteraction(IInteraction interaction)
		{
			_interactions.Add(interaction);
			interaction.Activated += Interaction_Activated;
			interaction.DeActivated += Interaction_DeActivated;
		}

		/// <summary>
		/// 'multicast' AddELement to all interactions
		/// </summary>
		public void AddElement(FrameworkElement element)
		{
			foreach (var interaction in _interactions)
			{
				interaction.AddElement(element);
			}
		}

		private void Interaction_DeActivated(object sender, EventArgs e)
		{
			// when an interactions is de-activated, re-enable all interactions
			foreach (var interaction in _interactions)
			{
				interaction.IsEnabled = true;
			}
		}

		private void Interaction_Activated(object sender, EventArgs e)
		{
			// when an interaction is activated, disable all others
			foreach (var interaction in _interactions.Where(i => i != sender))
			{
				interaction.IsEnabled = false;
			}
		}
	}
}
