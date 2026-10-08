using UnityEngine;

//comandi comuni ai controller del motorino (pulsanti a schermo, tastiera, GameManager)
public interface IBikeControls
{
    void Accellera();
    void Deaccellera();
    void Frena();
    void SterzaDx();
    void SterzaSx();
    void Desterza();
}

public static class BikeControls
{
    //restituisce il controller attivo sull'oggetto. Se è attivo SphereBikeController ha la precedenza:
    //in quel caso il vecchio controller resta acceso solo come burattino e riceve i comandi da lui
    public static IBikeControls FindActive(GameObject bike)
    {
        SphereBikeController sphere = bike.GetComponent<SphereBikeController>();
        if (sphere != null && sphere.enabled)
            return sphere;

        foreach (IBikeControls controls in bike.GetComponents<IBikeControls>())
        {
            if (controls is Behaviour behaviour && behaviour.enabled)
                return controls;
        }
        return null;
    }
}
