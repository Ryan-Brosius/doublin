
// Elements that exist in the game
// Prob will be moved or created by someone else so its fine if this gets deleted--- just pls have same name
public enum Element { None, Fire, Ice }

/*  Owner: Ryan Brosius
 * 
 *  Interface for a shield, contains the functions that a shield should have
 *  Didn't really need an interface for this, but its okay
 *  Look at Shield for reference on how to use this
 */
public interface IShield
{
    Element Element { get; }
    float Health { get; }
    bool IsActive { get; }
    bool AbsorbDamage(Damage damage, out float leftover);
    void Break();
}