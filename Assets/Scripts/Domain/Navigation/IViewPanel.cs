using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IViewPanel
{
    GameObject gameObject { get; }

    // Menggunakan UniTask agar bisa di-await (misal untuk animasi fade in/out)
    UniTask Show(CancellationToken token = default);
    UniTask Hide(CancellationToken token = default);

    // Untuk kebutuhan instant (misal: Reset state saat loading)
    void SetVisibilityImmediate(bool isVisible);

    // State check
    bool IsActive { get; }
}