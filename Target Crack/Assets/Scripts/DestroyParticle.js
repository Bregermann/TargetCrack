#pragma strict

//Removes the particle effect from the game after the particle finishes emitting
Destroy(gameObject, GetComponent.<ParticleSystem>().duration);
