using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Rigidbody2DExtension {

	public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius) {
        Vector3 direction = body.transform.position - explosionPosition;
        float rate = 1 - (direction.magnitude / explosionRadius);
        if (rate > 0) {
            body.AddForce(direction.normalized * explosionForce * rate);
        }
    }

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode) {
        Vector3 direction = body.transform.position - explosionPosition;
        float rate = 1 - (direction.magnitude / explosionRadius);
        if (rate > 0) {
            body.AddForce(direction.normalized * explosionForce * rate, mode);
        }
    }
}
