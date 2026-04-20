using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Interfaces;

public interface IHandNeighborAware
{
    CardModel? CapturedLeftNeighbor { get; set; }
    CardModel? CapturedRightNeighbor { get; set; }
}