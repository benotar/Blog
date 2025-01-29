const initialState = {
    currentUser: null,
    tokens: null,
    errorMessage: null,
    loading: false
};

export const createUserSlice = (set) => ({
    ...initialState,
    signInStart: () => {
        set((state) => ({
            ...state,
            loading: true,
            errorMessage: null
        }));
    },
    signInSuccess: (payload) => {
        set((state) => ({
            ...state,
            currentUser: payload.currentUser,
            tokens: payload.tokens,
            loading: false,
            errorMessage: null
        }));
    },
    signInFailure: (payload) => {
        set((state) => ({
            ...state,
            currentUser: null,
            tokens: null,
            loading: false,
            errorMessage: payload
        }));
    },
    updateStart: () => {
        set((state) => ({
            ...state,
            loading: true,
            errorMessage: null
        }));
    },
    updateSuccess: (payload) => {
        set((state) => ({
            ...state,
            currentUser: payload,
            loading: false,
            errorMessage: null
        }));
    },
    updateFailure: (payload) => {
        set((state) => ({
            ...state,
            loading: false,
            errorMessage: payload
        }));
    },
    deleteStart: () => {
        set((state) => ({
            ...state,
            loading: true,
            errorMessage: null
        }));
    },
    deleteSuccess: () => {
        set((state) => ({
            ...state,
            currentUser: null,
            tokens: null,
            loading: false,
            errorMessage: null
        }));
    },
    deleteFailure: (payload) => {
        set((state) => ({
            ...state,
            loading: false,
            errorMessage: payload
        }));
    },
    refreshStart: () => {
        set((state) => ({
            ...state,
            loading: true,
            errorMessage: null
        }));
    },
    refreshSuccess: (payload) => {
        set((state) => ({
            ...state,
            tokens: payload,
            loading: false,
            errorMessage: null
        }));
    },
    refreshFailure: (payload) => {
        set((state) => ({
            ...state,
            currentUser: null,
            tokens: null,
            loading: false,
            errorMessage: payload
        }));
    },
    doSignOut:() => {
        set((state) => ({
            ...state,
            currentUser: null,
            tokens: null,
            loading: false,
            errorMessage: null
        }));
    }
});

