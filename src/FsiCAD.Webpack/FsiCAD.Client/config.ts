import * as path from 'path';
import * as webpack from 'webpack';
import { BaseConfig } from '../baseconfig';
import { CssMinificationConfig } from '../minification.css';

const StylesPath: string = '../FsiCAD/Client/wwwroot/styles';
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
