const initialState = {
    theme: "light"
};

export const createThemeSlice = (set) => ({
    ...initialState,
    toggleTheme: () => {
        set((state) => ({
            theme: state.theme === "light" ? "dark" : "light"
        }));
    }
});