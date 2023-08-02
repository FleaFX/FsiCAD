import * as path from 'path';
import * as webpack from 'webpack';
import { BaseConfig } from '../baseconfig';
import { JsMinificationConfig } from '../minification.js';
import { CssMinificationConfig } from '../minification.css';

const ComponentsPath: string = '../FsiCAD.Components/wwwroot/scripts';

export const JsConfig: webpack.Configuration = {
    ...BaseConfig,
    ...JsMinificationConfig,
    entry: {
    },
    output: {
        ...BaseConfig.output,
        path: path.resolve(__dirname, `../${ComponentsPath}`)
    }
};

const StylesPath: string = '../FsiCAD.Components/wwwroot/styles';
export const CssConfig: webpack.Configuration = {
    ...BaseConfig,
    ...CssMinificationConfig,

    entry: {
        'app': `${StylesPath}/app.scss`,
        'app.min': `${StylesPath}/app.scss`
    },
    output: {
        ...BaseConfig.output,
        path: path.resolve(__dirname, `../${StylesPath}`)
    }
};
