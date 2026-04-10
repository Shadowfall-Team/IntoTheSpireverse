using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.ui;

public partial class NCargoPile : NCombatCardPile
{
	protected override PileType Pile => CargoCardPile.CargoPileType;
	
	public override void _Ready()
	{
		ConnectSignals();
		_emptyPileMessage = new LocString("combat_messages", "OPEN_EMPTY_CARGO");
		
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "CARGO_PILE.title"), new LocString("static_hover_tips", "CARGO_PILE.description"));
		
		
		Visible = false;
		SetAnimInOutPositions();
		Disable();
	}
	
	protected override void SetAnimInOutPositions()
	{
		_showPosition = Position;
		_hidePosition = Position + new Vector2(-150f, 0f);
	}

	public override void Initialize(Player player)
	{
		_localPlayer = player;
		_pile = Pile.GetPile(_localPlayer);
		_pile.ContentsChanged += HandleContentsChanged;
		
		_currentCount = _pile.Cards.Count;
		_countLabel.SetTextAutoSize(_currentCount.ToString());
		
		if (_pile.Cards.Count > 0)
		{
			Visible = true;
			Enable();
		}
	}

	private void HandleContentsChanged()
	{
		_currentCount = _pile.Cards.Count;
		_countLabel.SetTextAutoSize(_currentCount.ToString());
		
		if (_currentCount > 0 && !Visible)
		{
			AnimIn();
			Enable();
		}
		else if (_currentCount == 0 && Visible)
		{
			Disable();
		}
	}

	protected override void OnFocus()
	{
		var tooltip = NHoverTipSet.CreateAndShow(this, _hoverTip);
		tooltip.GlobalPosition = GlobalPosition + new Vector2(0, -300);
		_bumpTween?.Kill();
		_bumpTween = CreateTween();
		_bumpTween.TweenProperty(_icon, "scale", new Vector2(1.25f, 1.25f), 0.05);
	}

	public override void AnimIn()
	{
		base.AnimIn();
		Visible = true;
	}
}
