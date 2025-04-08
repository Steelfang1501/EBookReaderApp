import React from 'react'
import { Image, StyleSheet, Text, View, FlatList, TouchableOpacity, Dimensions } from 'react-native'

const {width: Screen_width, height: Screen_height} = Dimensions.get('window');

export default function BooksList({ navigation, Datas, Name }) {
    return (
        <View style = {{marginBottom: Screen_height*0.02}}>
            <View style = {{flexDirection: 'row', alignItems: 'center', justifyContent:'space-between',  margin: Screen_width*0.025,}}>
                <Text style={styles.text}>{Name}</Text>
                <TouchableOpacity>
                    <Text>More {'>'}</Text>
                </TouchableOpacity>
            </View>
            <FlatList data={Datas} numColumns={2} renderItem={({ item }) => {
                return (
                    <TouchableOpacity onPress={() => {
                        navigation.navigate('BookScreen', { item })
                    }}>
                        <View style={styles.container}>
                            <Image style={styles.image} source={item.img} resizeMode="cover" />
                            <Text numberOfLines={1} style={styles.textName}>{item.title}</Text>
                        </View>
                    </TouchableOpacity>
                )
            }} />
        </View>
    )
}
const styles = StyleSheet.create({
    container: {
        width: Screen_width*0.45,
        height: Screen_width*0.4,
        margin: Screen_width*0.025,
    },
    image: {
        width: '100%',
        height: '80%',
        borderRadius: 10,
    },
    text: {
        fontSize: 23,
        fontWeight: 500
    }
})