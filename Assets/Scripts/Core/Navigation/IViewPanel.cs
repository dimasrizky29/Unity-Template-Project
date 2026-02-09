using Cysharp.Threading.Tasks;
using System.Threading;

public interface IViewPanel
{
    // Menggunakan UniTask agar bisa di-await (misal untuk animasi fade in/out)
    UniTask Show(CancellationToken token = default);
    UniTask Hide(CancellationToken token = default);

    void SetVisibilityImmediate(bool isVisible);

    bool IsActive { get; }
}