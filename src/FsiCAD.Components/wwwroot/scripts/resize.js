var e={d:(t,n)=>{for(var r in n)e.o(n,r)&&!e.o(t,r)&&Object.defineProperty(t,r,{enumerable:!0,get:n[r]})},o:(e,t)=>Object.prototype.hasOwnProperty.call(e,t)},t={};function n(){document.querySelectorAll(".resizable .splitter").forEach((e=>{let t=!1;e.addEventListener("mousedown",(n=>{n.target==e&&(t=!0,e.parentElement?.classList.add("resizing"))})),document.addEventListener("mousemove",(n=>{if(!t||!e.parentElement)return!1;const r=n,o=e.parentElement.offsetLeft,a=r.clientX-o;e.parentElement.style.width=a+"px",e.parentElement.style.flexGrow="0"})),document.addEventListener("mouseup",(n=>{t=!1,e.parentElement?.classList.remove("resizing")}))}))}e.d(t,{R:()=>n});var r=t.R;export{r as addResize};