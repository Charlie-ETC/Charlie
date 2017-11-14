declare module 'rmwc/Button';
declare module 'rmwc/Drawer';
declare module 'rmwc/Icon';
declare module 'rmwc/IconButton';
declare module 'rmwc/List';
declare module 'rmwc/Toolbar';

interface ButtonProps {
    dense?: boolean;
    raised?: boolean;
    compact?: boolean;
    unelevated?: boolean;
    stroked?: boolean;
}

declare class Button extends React.Component<ButtonProps, any> {
}
