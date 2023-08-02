export function addResize() {
    document.querySelectorAll('.resizable .splitter').forEach((element: Element) => {
        let dragging = false;
        element.addEventListener('mousedown', (event: Event) => {
            if (event.target == element) {
                dragging = true;
                element.parentElement?.classList.add('resizing');
            }
        });

        document.addEventListener('mousemove', (event: Event) => {
            if (!dragging || !element.parentElement) return false;

            const mouseEvent = <MouseEvent>event;
            const offsetLeft = element.parentElement.offsetLeft;
            const pos = mouseEvent.clientX - offsetLeft;
            element.parentElement.style.width = pos + 'px';
            element.parentElement.style.flexGrow = '0';
        });

        document.addEventListener('mouseup', (event: Event) => {
            dragging = false;
            element.parentElement?.classList.remove('resizing');
        });
    });
}