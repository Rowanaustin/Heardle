export function PlaySound() {
    document.getElementById('audioPlayer').play();
}

export function StopSound() {
    document.getElementById('audioPlayer').pause();
    document.getElementById('audioPlayer').currentTime = 0;
}