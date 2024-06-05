using System.Collections;
using UnityEngine;

namespace CodeBase.Infrastructure.GameBoot {
  public interface ICoroutineHandler {
    Coroutine StartCoroutine(IEnumerator coroutine);
    void StopCoroutine(Coroutine routine);
  }
}