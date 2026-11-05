namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

/// <summary>
/// Lets any character, alt or normal, set the Max HP change for taking Parallel Stone in Mirror Mirror.
/// Negative loses Max HP, positive gains it, zero changes nothing and hides the Max HP text.
/// See MirrorMirror.GetParallelStoneMaxHpChange.
/// Added to support both the production environment issues Mango called out, and also support the existence
/// of Spireverse expansion mods (alt Defect farmer guy on the workshop)
/// 
/// To use it from another mod, add the interface to your CharacterModel and return your value:
/// <code>
/// public class MyCharacter : CharacterModel, IParallelStoneCharacter
/// {
///     public int ParallelStoneMaxHpChange => -4;
/// }
/// </code>
/// </summary>
public interface IParallelStoneCharacter
{
    public int ParallelStoneMaxHpChange { get; }
}
