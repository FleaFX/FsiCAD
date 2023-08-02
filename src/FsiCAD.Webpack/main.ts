import * as ComponentsConfig from './FsiCAD.Components/config';
import * as ClientConfig from './FsiCAD.Client/config';

export default [
    { ...ComponentsConfig.JsConfig, name: 'FsiCAD.Components.Js' },
    { ...ComponentsConfig.CssConfig, name: 'FsiCAD.Components.Css' },
    { ...ClientConfig.CssConfig, name: 'FsiCAD.Client.Css' }
];
