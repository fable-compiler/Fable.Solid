import{U as d,e as g,t as C,c as m,j as $,d as _,s as w,i as f,k as v,a as b,l as k,F as S,f as L,g as u}from"./index-CiwxCZNk.js";import{e as R}from"./List-biTribfy.js";const i={None:0,LowerFirst:1,SnakeCase:2,SnakeCaseAllCaps:3,KebabCase:4};function p(t,n){return t.replace(/[a-z]?[A-Z]/g,r=>r.length===1?r.toLowerCase():r.charAt(0)+n+r.charAt(1).toLowerCase())}function y(t,n){switch(n){case i.LowerFirst:return t.charAt(0).toLowerCase()+t.slice(1);case i.SnakeCase:return p(t,"_");case i.SnakeCaseAllCaps:return p(t,"_").toUpperCase();case i.KebabCase:return p(t,"-");case i.None:default:return t}}function A(t,n=i.None){const r={},s=n;function a(e){throw new Error("Cannot infer key and value of "+String(e))}function l(e,o,c){e=y(e,o),r[e]=c}for(let e of t){let o=i.None;if(e==null&&a(e),e instanceof d){const c=e.cases()[e.tag];e=e.fields.length===0?c:[c].concat(e.fields),o=s}if(Array.isArray(e))switch(e.length){case 0:a(e);break;case 1:l(e[0],o,!0);break;case 2:const c=e[1];l(e[0],o,c);break;default:l(e[0],o,e.slice(1))}else typeof e=="string"?l(e,o,!0):a(e)}return r}class h extends d{constructor(n,r){super(),this.tag=n,this.fields=r}cases(){return["Ok","Error"]}}function E(t){return t.then(n=>new h(0,[n]),n=>new h(1,[n]))}function F(t){return g(t.status)+" "+t.statusText+" for URL "+t.url}function U(t,n){return fetch(t,A(n,1)).then(s=>{if(s.ok)return s;throw new Error(F(s))})}function N(t,n){return E(U(t,n))}var j=u("<label>UserID: "),x=u('<input type=number placeholder="Enter Numeric Id">'),I=u("<ul>"),K=u("<li>Loading..."),O=u("<li><strong>");C("Loading Resource.fs...");function P(t){return N(`https://swapi.dev/api/people/${t}/`,R()).then(r=>r.tag===1?Promise.resolve({error:r.fields[0].message}):r.fields[0].json())}function D(){const t=m(void 0),n=$(t[0],P,{})[0];return[j(),(()=>{var r=x();return r.$$input=s=>{const a=s.target;t[1](a.value)},_(()=>w(r,"min",g(1))),r})(),(()=>{var r=I();return f(r,(()=>{var s=v(()=>!!n.loading);return()=>s()?K():b(S,{get each(){return Object.keys(n()||{}).map(a=>[a,n()[a]])},children:(a,l)=>(()=>{var e=O(),o=e.firstChild;return f(o,()=>a[0]),f(e,()=>`: ${k(a[1])}`,null),e})()})})()),r})()]}L(["input"]);export{D as Components__Components_Resource_Static,P as fetchUser};
